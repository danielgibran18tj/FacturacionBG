export interface ColumnDefinition {
  key: string; 
  label: string;
  type?: 'text' | 'date' | 'number' | 'status';
  format?: string;
  textAlign?: 'left' | 'center' | 'right';
}

export interface ActionDefinition {
  label: string;
  icon: string; 
  class: string;
  action: (item: any) => void; 
}