import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddViewProductsComponent } from './add-view-products.component';

describe('AddViewProductsComponent', () => {
  let component: AddViewProductsComponent;
  let fixture: ComponentFixture<AddViewProductsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddViewProductsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddViewProductsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
