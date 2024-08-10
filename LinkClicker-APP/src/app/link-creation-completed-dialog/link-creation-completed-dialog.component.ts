import { Component, OnInit, Input } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-link-creation-completed-dialog',
  templateUrl: './link-creation-completed-dialog.component.html',
  styleUrl: './link-creation-completed-dialog.component.css'
})
export class LinkCreationCompletedDialogComponent {
  @Input() position: 'bottom-left' | 'bottom-right' = 'bottom-left';
  visible: boolean = true;
  autoCloseTimeout: any;

  constructor(public dialogRef: MatDialogRef<LinkCreationCompletedDialogComponent>) {}

  ngOnInit(): void {
    this.autoCloseTimeout = setTimeout(() => {
      this.close();
    }, 4000);
  }

  close(): void {
    clearTimeout(this.autoCloseTimeout);
    this.visible = false;
    this.dialogRef.close();
  }
}
