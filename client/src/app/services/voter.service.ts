import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Voter } from 'src/models/Voter';

@Injectable({
  providedIn: 'root'
})
export class VoterService {
  private baseUrl = 'http://localhost:5000/api/voter';

  constructor(private http: HttpClient) {}

  registerVoter(voter: Voter): Observable<Voter> {
    return this.http.post<Voter>(`${this.baseUrl}/register-voter`, voter);
  }

  getVotersByCandidateNationalId(nationalId: string): Observable<Voter> {
    return this.http.get<Voter>(`${this.baseUrl}/get-voters-by-candidate-national-id/${nationalId}`);
  }

  getVotersByCandidateId(id: string): Observable<Voter> {
    return this.http.get<Voter>(`${this.baseUrl}/get-voters-by-candidate-id/${id}`);
  }

  getAllVoters(): Observable<Voter> {
    return this.http.get<Voter>(`${this.baseUrl}/get-all-voters`);
  }
}
