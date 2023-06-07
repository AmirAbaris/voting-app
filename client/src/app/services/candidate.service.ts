import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PresidentCandidate } from 'src/models/PresidentCandidate';

@Injectable({
  providedIn: 'root'
})
export class CandidateService {
  private apiBaseUrl = 'http://localhost:5000/';

  // Inject httpClient
  constructor(private httpClient: HttpClient) { }

  // Register a new candidate
  public registerCandidate(candidate: PresidentCandidate): Observable<PresidentCandidate> {
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };

    return this.httpClient.post<PresidentCandidate>(this.apiBaseUrl + 'register-candidate', candidate, httpOptions);
  }

  // Get candidate by national ID
  public getCandidateByNationalId(nationalId: string): Observable<PresidentCandidate> {
    return this.httpClient.get<PresidentCandidate>(this.apiBaseUrl + 'get-candidate-by-national-id/' + nationalId);
  }

  // Get candidate by ID
  public getCandidateById(id: string): Observable<PresidentCandidate> {
    return this.httpClient.get<PresidentCandidate>(this.apiBaseUrl + 'get-candidate-by-id/' + id);
  }

  // Get all candidates
  public getAllCandidates(): Observable<PresidentCandidate> {
    return this.httpClient.get<PresidentCandidate>(this.apiBaseUrl + 'get-candidates');
  }

  // Update candidate by national ID
  public updateCandidateByNationalId(
    nationalId: string, candidateIn: PresidentCandidate
  ): Observable<any> {
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    }

    return this.httpClient.put<any>(this.apiBaseUrl + 'update-candidate-by-national-id/' + nationalId, candidateIn, httpOptions);
  }

    // Delete candidate by national ID
    public deleteCandidateByNationalId(nationalId: string): Observable<any> {
      return this.httpClient.delete<any>(this.apiBaseUrl + 'delete-candidate-by-national-id/' + nationalId);
    }
}
