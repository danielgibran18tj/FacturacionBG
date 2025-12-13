export interface SystemSetting {
  id: number;
  key: string;
  value: string;
  description: string;
  dataType: 'string' | 'number' | 'decimal' | 'boolean';
  isSystem: boolean;
}
