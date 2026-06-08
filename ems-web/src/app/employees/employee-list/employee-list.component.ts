import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { EmployeeService, PagedResult } from '../../core/services/employee.service';
import { Employee } from '../../models/employee.model';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employee-list.component.html'
})
export class EmployeeListComponent implements OnInit, OnDestroy
{
  private employeeService = inject(EmployeeService);
  private router = inject(Router);
  private loadSub?: Subscription;
  private deleteSub?: Subscription;

  employees: Employee[] = [];
  isLoading = false;
  deletingId: number | null = null;
  errorMessage: string | null = null;
  page = 1;
  pageSize = 10;
  total = 0;

  ngOnInit(): void
  {
    this.loadEmployees();
  }

  loadEmployees(): void
  {
    this.isLoading = true;
    this.errorMessage = null;
    this.loadSub?.unsubscribe();
    this.loadSub = this.employeeService.getAll(this.page, this.pageSize)
      .subscribe({
        next: (result: PagedResult<Employee>) =>
        {
          this.employees = result.data;
          this.total = result.total;
          this.isLoading = false;
        },
        error: () =>
        {
          this.errorMessage = 'Failed to load employees. Please try again.';
          this.isLoading = false;
        }
      });
  }

  onAdd(): void
  {
    this.router.navigate(['/employees/new']);
  }

  onEdit(id: number): void
  {
    this.router.navigate(['/employees', id, 'edit']);
  }

  onDelete(id: number): void
  {
    if (!confirm('Are you sure you want to delete this employee?'))
    {
      return;
    }
    this.deletingId = id;
    this.deleteSub = this.employeeService.delete(id)
      .subscribe({
        next: () =>
        {
          this.deletingId = null;
          this.loadEmployees();
        },
        error: () =>
        {
          this.deletingId = null;
          this.errorMessage = 'Failed to delete employee. Please try again.';
        }
      });
  }

  ngOnDestroy(): void
  {
    this.loadSub?.unsubscribe();
    this.deleteSub?.unsubscribe();
  }

  get totalPages(): number
  {
    return Math.ceil(this.total / this.pageSize);
  }

  get pageNumbers(): number[]
  {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  onPageChange(newPage: number): void
  {
    this.page = newPage;
    this.loadEmployees();
  }

  onPageSizeChange(event: Event): void
  {
    this.pageSize = Number((event.target as HTMLSelectElement).value);
    this.page = 1;
    this.loadEmployees();
  }
}
