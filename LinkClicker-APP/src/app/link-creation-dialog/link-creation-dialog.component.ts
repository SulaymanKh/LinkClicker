import { Component, OnInit, Input } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-link-creation-dialog',
  templateUrl: './link-creation-dialog.component.html',
  styleUrl: './link-creation-dialog.component.css'
})
export class LinkCreationDialogComponent implements OnInit {
  @Input() position: 'bottom-left' | 'bottom-right' = 'bottom-right';
  visible: boolean = true;
  autoCloseTimeout: any;

  constructor(public dialogRef: MatDialogRef<LinkCreationDialogComponent>) {}

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
