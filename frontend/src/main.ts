import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { provideHttpClient } from '@angular/common/http'; // Import provideHttpClient
bootstrapApplication(AppComponent, {
  ...appConfig,
  providers: [
    ...appConfig.providers, // Spread existing providers from appConfig
    provideHttpClient() // Add provideHttpClient to providers array
  ]
})
  .catch((err) => console.error(err));
