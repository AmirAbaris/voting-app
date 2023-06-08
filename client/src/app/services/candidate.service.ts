import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PresidentCandidate } from 'src/models/PresidentCandidate';

@Injectable({
  providedIn: 'root'
})
export class CandidateService {
  private baseUrl = 'http://localhost:5000/api/presidentcandidate';

  constructor(private http: HttpClient) {}

  registerCandidate(candidate: PresidentCandidate): Observable<PresidentCandidate> {
    return this.http.post<PresidentCandidate>(`${this.baseUrl}/register-candidate`, candidate);
  }

  getCandidateByNationalId(nationalId: string): Observable<PresidentCandidate> {
    return this.http.get<PresidentCandidate>(`${this.baseUrl}/get-candidate-by-national-id/${nationalId}`);
  }

  getAllCandidates(): Observable<PresidentCandidate[]> {
    return this.http.get<PresidentCandidate[]>(`${this.baseUrl}/get-candidates`);
  }

  updateCandidate(nationalId: string, candidate: PresidentCandidate): Observable<any> {
    return this.http.put(`${this.baseUrl}/update-candidate-by-national-id/${nationalId}`, candidate);
  }

  deleteCandidate(nationalId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/delete-candidate-by-national-id/${nationalId}`);
  }
}
