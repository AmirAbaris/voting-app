import { Gender } from "src/Enums/Gender";
import { Address } from "./Address";

export interface Voter {
    voterId?: string;
    voterNationalId: string;
    selectedPresidentByID: string;
    SelectedPresidentNationalId: string;
    FirstName: string;
    LastName: string;
    gender: Gender;
    age: number;
    address: Address;
}