import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'employees', pathMatch: 'full' },
  {
    path: 'employees',
    loadComponent: () => import('./employees/employee-list/employee-list.component')
      .then(m => m.EmployeeListComponent)
  },
  {
    path: 'employees/new',
    loadComponent: () => import('./employees/employee-form/employee-form.component')
      .then(m => m.EmployeeFormComponent)
  },
  {
    path: 'employees/:id/edit',
    loadComponent: () => import('./employees/employee-form/employee-form.component')
      .then(m => m.EmployeeFormComponent)
  }
];
