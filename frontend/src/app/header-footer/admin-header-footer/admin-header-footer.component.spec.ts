import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminHeaderFooterComponent } from './admin-header-footer.component';

describe('AdminHeaderFooterComponent', () => {
  let component: AdminHeaderFooterComponent;
  let fixture: ComponentFixture<AdminHeaderFooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminHeaderFooterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminHeaderFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
