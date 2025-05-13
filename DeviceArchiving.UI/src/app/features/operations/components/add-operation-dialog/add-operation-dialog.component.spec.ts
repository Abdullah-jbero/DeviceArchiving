import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddOperationDialogComponent } from './add-operation-dialog.component';

describe('AddOperationDialogComponent', () => {
  let component: AddOperationDialogComponent;
  let fixture: ComponentFixture<AddOperationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AddOperationDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddOperationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
