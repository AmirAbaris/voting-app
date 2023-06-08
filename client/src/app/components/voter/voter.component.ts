import { Component, OnInit } from '@angular/core';
import { Gender } from 'src/Enums/Gender';
import { State } from 'src/Enums/State';
import { VoterService } from 'src/app/services/voter.service';
import { Voter } from 'src/models/Voter';

@Component({
  selector: 'app-voter',
  templateUrl: './voter.component.html',
  styleUrls: ['./voter.component.scss']
})
export class VoterComponent implements OnInit {
  voters: Voter[] = [];
  newVoter: Voter = {
    voterNationalId: '',
    selectedPresidentById: '',
    SelectedPresidentNationalId: '',
    FirstName: '',
    LastName: '',
    gender: Gender.Female,
    age: 0,
    address: {
      city: '',
      state: State.Hawaii,
      street: '',
      zipCode: ''
    }
  }

  constructor(private voterService: VoterService) {}

  ngOnInit(): void {
      this.getAllVoters()
  }

  getAllVoters(): void {
    this.voterService.getAllVoters().subscribe((voters) => {
      this.voters.push(voters);
    })
  }

  registerVoter(): void {
    this.voterService.registerVoter(this.newVoter).subscribe((voter) => {
      console.log('Voter registered successfully', voter);
      this.getAllVoters();
      this.newVoter = {
        voterNationalId: this.newVoter.voterNationalId,
        selectedPresidentById: this.newVoter.selectedPresidentById,
        SelectedPresidentNationalId: this.newVoter.SelectedPresidentNationalId,
        FirstName: this.newVoter.FirstName,
        LastName: this.newVoter.LastName,
        gender: this.newVoter.gender,
        age: this.newVoter.age,
        address: {
          city: this.newVoter.address.city,
          state: this.newVoter.address.state,
          street: this.newVoter.address.state,
          zipCode: this.newVoter.address.zipCode
        }
      }
    })
  }

  getVotersByCandidateNationalId(nationalId: string): void {
    this.voterService.getVotersByCandidateNationalId(nationalId).subscribe((voters) => {
      console.log("Got Voters:", voters);
    });
  }

  getVotersByCandidateId(id: string): void {
    this.voterService.getVotersByCandidateId(id).subscribe((voters) =>
    console.log("Got Voters:", voters));
  }
}
