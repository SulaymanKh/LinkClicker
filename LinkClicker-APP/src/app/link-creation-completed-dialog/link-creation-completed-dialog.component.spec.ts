import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkCreationCompletedDialogComponent } from './link-creation-completed-dialog.component';

describe('LinkCreationCompletedDialogComponent', () => {
  let component: LinkCreationCompletedDialogComponent;
  let fixture: ComponentFixture<LinkCreationCompletedDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LinkCreationCompletedDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LinkCreationCompletedDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
