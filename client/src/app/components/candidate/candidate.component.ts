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
  candidates: PresidentCandidate[] = [];
  newCandidate: PresidentCandidate = {
    candidateNationalId: '',
    firstName: '',
    lastName: '',
    age: 0,
    gender: Gender.Male,
    party: Party.Republican,
    address: {
      city: '',
      state: State.Hawaii,
      street: '',
      zipCode: ''
    }
  };
PresidentCandidate: any;
Gender: any;
Party: any;
State: any;

  constructor(private candidateService: CandidateService) {}

  ngOnInit(): void {
    this.getAllCandidates();
  }

  getAllCandidates(): void {
    this.candidateService.getAllCandidates().subscribe((candidates) => {
      this.candidates = candidates;
    });
  }

  registerCandidate(): void {
    this.candidateService.registerCandidate(this.newCandidate).subscribe((candidate) => {
      console.log('Candidate registered successfully', candidate);
      this.getAllCandidates();
      this.newCandidate = {
        candidateNationalId: this.newCandidate.candidateNationalId,
        firstName: this.newCandidate.firstName,
        lastName: this.newCandidate.lastName,
        age: this.newCandidate.age,
        gender: this.newCandidate.gender,
        party: this.newCandidate.party,
        address: {
          city: this.newCandidate.address.city,
          state: this.newCandidate.address.state,
          street: this.newCandidate.address.street,
          zipCode: this.newCandidate.address.zipCode
        }
      };
    });
  }

  deleteCandidate(nationalId: string): void {
    this.candidateService.deleteCandidate(nationalId).subscribe(() => {
      console.log('Candidate deleted successfully');
      this.getAllCandidates();
    });
  }
}