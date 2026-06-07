import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Employee, EmployeeDto } from '../../models/employee.model';

export interface PagedResult<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/employees`;

  getAll(page = 1, pageSize = 10): Observable<PagedResult<Employee>> {
    return this.http.get<PagedResult<Employee>>(`${this.apiUrl}?page=${page}&pageSize=${pageSize}`)
      .pipe(catchError(err => throwError(() => err)));
  }

  getById(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/${id}`)
      .pipe(catchError(err => throwError(() => err)));
  }

  create(dto: EmployeeDto): Observable<Employee> {
    return this.http.post<Employee>(this.apiUrl, dto)
      .pipe(catchError(err => throwError(() => err)));
  }

  update(id: number, dto: EmployeeDto): Observable<Employee> {
    return this.http.put<Employee>(`${this.apiUrl}/${id}`, dto)
      .pipe(catchError(err => throwError(() => err)));
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError(err => throwError(() => err)));
  }
}
