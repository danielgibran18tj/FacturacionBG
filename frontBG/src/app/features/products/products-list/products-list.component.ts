import { Component } from '@angular/core';
import { GenericTableComponent } from "../../../shared/components/generic-table/generic-table.component";
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { CreateProductModalComponent } from "../create-product-modal/create-product-modal.component";
import { CommonModule } from '@angular/common';
import { EditProductModalComponent } from "../edit-product-modal/edit-product-modal.component";

@Component({
  selector: 'app-products-list',
  imports: [GenericTableComponent, FormsModule, CreateProductModalComponent, CommonModule, EditProductModalComponent],
  templateUrl: './products-list.component.html',
  styleUrl: './products-list.component.css'
})
export class ProductsListComponent {
  // Datos de la búsqueda
  searchTerm = "";
  minStock: number = 0;
  maxStock: number = 1000;
  minPrice: number = 0;
  maxPrice: number = 10000;

  // Datos de productos
  products: any[] = [];
  totalPages: number = 1;
  page: number = 1;

  productColumns: any[] = [];
  productActions: any[] = [];

  selectedProduct: any = null;
  showEditProduct: boolean = false;
  showDelete: boolean = false;
  showCreateProduct: boolean = false;

  constructor(private productService: ProductService) {
    this.defineTableStructure();
    this.loadProducts();
  }

  // Definir columnas y acciones para el componente genérico de tabla
  defineTableStructure() {
    this.productColumns = [
      { key: 'code', label: 'Código', textAlign: 'left' },
      { key: 'name', label: 'Nombre', textAlign: 'left' },
      { key: 'unitPrice', label: 'Precio Unitario', type: 'number', format: '1.2-2', textAlign: 'right' },
      { key: 'stock', label: 'Stock', textAlign: 'center' },
      { key: 'isActive', label: 'Estado', type: 'isActive', textAlign: 'center' }
    ];

    this.productActions = [
      {
        label: 'Editar',
        icon: 'bi-pencil',
        class: 'btn-secondary',
        action: (product: any) => this.openEdit(product)
        // },
        // {
        //   label: 'Eliminar',
        //   icon: 'bi-trash',
        //   class: 'btn-danger',
        //   action: (product: any) => this.openDelete(product)
      }
    ];
  }

  // Cargar productos
  loadProducts() {
    const params = {
      SearchTerm: this.searchTerm,
      Page: this.page.toString(),
      PageSize: '5',
      // MinStock: this.minStock.toString(),
      // MaxStock: this.maxStock.toString(),
      // MinPrice: this.minPrice.toString(),
      // MaxPrice: this.maxPrice.toString()
    };

    console.log(params);

    this.productService.getPagedProducts({ params })
      .subscribe((response: any) => {
        this.products = response.items;
        this.totalPages = response.totalPages;
      });
  }

  onSearch() {
    this.page = 1;      // Reinicia la paginación
    this.loadProducts();
  }

  // Manejar cambio de página desde la tabla
  handlePageChange(newPage: number) {
    this.page = newPage;
    this.loadProducts();
  }

  // Manejar clic en las acciones
  handleActionClick(event: { action: (item: any) => void, item: any }) {
    event.action(event.item);
  }

  // Abrir detalles de producto
  openEdit(product: any) {
    this.selectedProduct = product;
    this.showEditProduct = true;
  }

  // Cerrar detalles de producto
  closeDetails() {
    this.showEditProduct = false;
    this.selectedProduct = null;
  }

  // Abrir modal de eliminación
  openDelete(product: any) {
    this.selectedProduct = product;
    this.showDelete = true;
  }

  // Cerrar modal de eliminación
  closeDelete() {
    this.showDelete = false;
    this.selectedProduct = null;
  }

  updateProduct(event: { id: number; body: any }) {
    this.productService.update(event.id, event.body).subscribe(() => {
      this.showEditProduct = false;
      this.loadProducts();
    });
  }

  // Eliminar producto
  deleteProduct(product: any) {
    console.log("Aquí va la lógica de eliminacion");
    // this.http.delete(`https://localhost:7223/api/Product/${product.id}`).subscribe(() => {
    //   this.loadProducts();
    //   this.closeDelete();
    // });
  }

  // Abrir modal para crear producto
  openCreateProduct() {
    this.showCreateProduct = true;
  }

  createProduct(body: any) {
    this.productService.create(body).subscribe(() => {
      this.showCreateProduct = false;
      this.loadProducts();
    });
  }

  // Aplicar filtros
  applyFilters() {
    this.page = 1;
    this.loadProducts();
  }

  // Limpiar filtros
  clearFilters() {
    this.minStock = 0;
    this.maxStock = 1000;
    this.minPrice = 0;
    this.maxPrice = 10000;
    this.loadProducts();
  }
}
