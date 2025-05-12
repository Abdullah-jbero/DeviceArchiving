import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationTypeFormComponent } from './operation-type-form.component';

describe('OperationTypeFormComponent', () => {
  let component: OperationTypeFormComponent;
  let fixture: ComponentFixture<OperationTypeFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [OperationTypeFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OperationTypeFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
