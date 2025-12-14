export interface Customer {
  id: number;
  fullName: string;
  phone: string;
  email: string;
  address: string;
  isActive: boolean;
}

export interface CreateCustomerRequest {
  documentNumber: string;
  fullName: string;
  phone: string;
  email: string;
  address: string;
  hasUserAccount?: boolean;
  username?: string;
}