import { Component, OnInit } from '@angular/core';
import { CandidateService } from 'src/app/services/candidate.service'; 
import { PresidentCandidate } from 'src/models/PresidentCandidate';
import { Gender } from 'src/Enums/Gender';
import { Party } from 'src/Enums/Party';
import { State } from 'src/Enums/State';

@Component({
  selector: 'app-candidate',
  templateUrl: './candidate.component.html',
  styleUrls: ['./candidate.component.scss']
})
export class CandidateComponent implements OnInit {
  candidates: PresidentCandidate[] = []
  newCandidate: PresidentCandidate = {
    candidateNationalId: '',
    firstName: '',
    lastName: '',
    age: 0,
    gender: Gender.Male,
    party: Party.Republican,
    address: {
      city: '',
      state: State.California,
      street: '',
      zipCode: ''
    }
  };

  constructor(private candidateService: CandidateService) { }

  ngOnInit(): void {
    this.getAllCandidates();
  }

  getAllCandidates(): void {
    this.candidateService.getAllCandidates()
      .subscribe((candidate: PresidentCandidate) => {
        this.candidates.push(candidate);
      });
  }  

  registerCandidate(): void {
    this.candidateService.registerCandidate(this.newCandidate).subscribe((candidate) => {
      console.log('Candidate registered successfully', candidate);
      this.getAllCandidates();
      this.newCandidate = {
        candidateNationalId: candidate.candidateNationalId,
        firstName: candidate.firstName,
        lastName: candidate.lastName,
        age: candidate.age,
        gender: candidate.gender,
        party: candidate.party,
        address: {
          city: candidate.address.city,
          state: candidate.address.state,
          street: candidate.address.state,
          zipCode: candidate.address.zipCode
        }
      };
    });
  }


  deleteCandidate(nationalId: string): void {
    this.candidateService.deleteCandidateByNationalId(nationalId).subscribe(
      () => {
        console.log('Candidate deleted successfully');
        this.getAllCandidates();
      },
      (error: any) => {
        console.log(error);
      }
    );
  }
}