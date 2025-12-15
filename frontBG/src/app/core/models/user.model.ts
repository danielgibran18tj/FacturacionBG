export interface User {
  id: number;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  fullName?: string;
}

export interface UpdateUserRequest {
  email: string;
  firstName: string;
  lastName: string;
  isActive: boolean;
}