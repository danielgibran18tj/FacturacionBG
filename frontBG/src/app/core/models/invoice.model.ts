export interface Invoice {
  id: number;
  invoiceNumber: string;
  invoiceDate: string;            // viene como string ISO
  customerName: string;
  customerDocument: string;
  sellerName: string;

  subtotal: number;
  taxIva: number;
  total: number;

  notes: string;
  status: string;

  details: InvoiceDetail[];
  payments: InvoicePayment[];
}

export interface InvoiceDetail {
  id: number;
  productId: number;
  productCode: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface InvoicePayment {
  paymentMethodId: number;
  paymentMethodName: string;
  amount: number;
}

export interface PagedInvoiceResult {
  items: Invoice[];
  totalItems: number;
  totalPages: number;
  page: number;
  pageSize: number;
}
