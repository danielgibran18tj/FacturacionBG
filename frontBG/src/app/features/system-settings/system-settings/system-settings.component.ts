import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SystemSettingsService } from '../../../core/services/system-settings.service';

@Component({
  selector: 'app-system-settings',
  imports: [FormsModule, CommonModule],
  templateUrl: './system-settings.component.html',
  styleUrl: './system-settings.component.css'
})
export class SystemSettingsComponent {
  settings: any[] = [];
  savingKey: string | null = null;

  constructor(private systemSettingsService: SystemSettingsService) { }

  ngOnInit() {
    this.loadSettings();
  }

  loadSettings() {
    this.systemSettingsService.getAll().subscribe(res => {
      this.settings = res;
    });
  }

  onBooleanChange(event: Event, setting: any) {
    const input = event.target as HTMLInputElement;
    setting.value = input.checked.toString();
  }


  save(setting: any) {
    this.savingKey = setting.key;

    this.systemSettingsService.updateSetting(setting.key, setting.value)
      .subscribe({
        next: () => this.savingKey = null,
        error: () => this.savingKey = null
      });
  }
}
