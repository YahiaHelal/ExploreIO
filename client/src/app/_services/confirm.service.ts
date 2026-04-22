import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { initialState } from 'ngx-bootstrap/timepicker/reducer/timepicker.reducer';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';
import { Observable, Observer } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModalRef?: BsModalRef;

  constructor(private modalService: BsModalService) { }

  confirm(title = 'Confirmation',
      msg = 'Are you sure you want to proceed ?',
      btnOkText = 'Ok',
      btnCancelText = 'Cancel'): Observable<boolean> {
    const config = {
      initialState: {
        title,
        msg,
        btnOkText,
        btnCancelText
      }
    }
    this.bsModalRef = this.modalService.show(ConfirmDialogComponent, config);
    return new Observable<boolean>(this.getResult());
  }

  getResult() {
    return (observer) => {
      const sub = this.bsModalRef?.onHidden?.subscribe(() => {
        observer.next(this.bsModalRef?.content.result);
        observer.complete();
      });
      return {
        unsubscribe() {
          sub?.unsubscribe();
        }
      }
    }
  }

}
