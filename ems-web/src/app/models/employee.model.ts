export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string | null;
  department: string;
  position: string;
  salary: number;
}

export type EmployeeDto = Omit<Employee, 'id'>;
