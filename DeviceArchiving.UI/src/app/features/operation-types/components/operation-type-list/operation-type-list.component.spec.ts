import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationTypeListComponent } from './operation-type-list.component';

describe('OperationTypeListComponent', () => {
  let component: OperationTypeListComponent;
  let fixture: ComponentFixture<OperationTypeListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [OperationTypeListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OperationTypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
