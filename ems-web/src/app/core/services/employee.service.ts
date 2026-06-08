import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Employee, EmployeeDto } from '../../models/employee.model';

export interface PagedResult<T>
{
  data: T[];
  total: number;
  page: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class EmployeeService
{
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/employees`;

  getAll(page = 1, pageSize = 10): Observable<PagedResult<Employee>>
  {
    return this.http.get<PagedResult<Employee>>(`${this.apiUrl}?page=${page}&pageSize=${pageSize}`);
  }

  getById(id: number): Observable<Employee>
  {
    return this.http.get<Employee>(`${this.apiUrl}/${id}`);
  }

  create(dto: EmployeeDto): Observable<Employee>
  {
    return this.http.post<Employee>(this.apiUrl, dto);
  }

  update(id: number, dto: EmployeeDto): Observable<Employee>
  {
    return this.http.put<Employee>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void>
  {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
