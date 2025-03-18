import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersAdminInfoComponent } from './users-admin-info.component';

describe('UsersAdminInfoComponent', () => {
  let component: UsersAdminInfoComponent;
  let fixture: ComponentFixture<UsersAdminInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsersAdminInfoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UsersAdminInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
