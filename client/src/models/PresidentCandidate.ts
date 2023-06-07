import { Gender } from "src/Enums/Gender";
import { Address } from "./Address";
import { Party } from "src/Enums/Party";

export interface PresidentCandidate {
    candidateId?: string;
    candidateNationalId: string;
    firstName: string;
    lastName: string;
    age: number;
    gender: Gender;
    party: Party;
    address: Address;
}