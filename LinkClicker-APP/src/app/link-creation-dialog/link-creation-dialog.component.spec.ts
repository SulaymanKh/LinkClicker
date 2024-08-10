import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkCreationDialogComponent } from './link-creation-dialog.component';

describe('LinkCreationDialogComponent', () => {
  let component: LinkCreationDialogComponent;
  let fixture: ComponentFixture<LinkCreationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LinkCreationDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LinkCreationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
