import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeveloperNavbarComponent } from './developer-navbar.component';

describe('DeveloperNavbarComponent', () => {
  let component: DeveloperNavbarComponent;
  let fixture: ComponentFixture<DeveloperNavbarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeveloperNavbarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeveloperNavbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
