import { State } from "src/Enums/State";

export interface Address {
    city: string;
    state: State;
    street: string;
    zipCode: string;
}