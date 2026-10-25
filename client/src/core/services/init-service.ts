import { inject, Service } from '@angular/core';
import { AccountService } from './account-service';
import { Observable, of } from 'rxjs';

@Service()
export class InitService {
    private accountService = inject(AccountService);

    init() {
        const useerString = localStorage.getItem('user');
        if (!useerString) return of(null);
        const user = JSON.parse(useerString);
        this.accountService.currentUser.set(user);

        return of(null);
    }
}
