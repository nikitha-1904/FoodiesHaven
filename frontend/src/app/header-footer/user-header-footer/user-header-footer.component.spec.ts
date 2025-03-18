import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserHeaderFooterComponent } from './user-header-footer.component';

describe('UserHeaderFooterComponent', () => {
  let component: UserHeaderFooterComponent;
  let fixture: ComponentFixture<UserHeaderFooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserHeaderFooterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserHeaderFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
