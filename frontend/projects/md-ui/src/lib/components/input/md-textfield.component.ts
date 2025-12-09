import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, SimpleChanges, ViewChild, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EdgeInsets } from '../../models';
import { edgeInsetsToStyle } from '../../utils';

@Component({
  selector: 'md-textfield',
  template: `
    <div [class]="customClasses" [style]="customCss" [style.margin]="marginStyle" [style.padding]="paddingStyle">
      @if (label) {
        <label class="form-label">{{ label }}</label>
      }
      <div class="input-group">
        <input 
          #inputElement
          class="form-control"
          [type]="inputType"
          [value]="localValue"
          (input)="onInput($event)"
          [placeholder]="placeholder || ''"
          [disabled]="disabled"
          [attr.maxlength]="maxLength"
          (blur)="onBlur()"
          (keydown.enter)="onEnter($event)" />
        @if (sensitive) {
          <button 
            class="btn btn-outline-secondary password-toggle" 
            type="button"
            (click)="togglePasswordVisibility()"
            [disabled]="disabled"
            [attr.aria-label]="showPassword ? 'Hide password' : 'Show password'">
            <i [class]="showPassword ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye'"></i>
          </button>
        }
      </div>
    </div>
  `,
  styles: [`
    .input-group { position: relative; }
    .password-toggle { border-left: 0; padding: 0.375rem 0.75rem; display: flex; align-items: center; justify-content: center; min-width: 42px; }
    .input-group input { border-right: 0; }
    .input-group input:focus + .btn { border-color: #86b7fe; }
  `],
  standalone: true,
  imports: [FormsModule]
})
export class MdTextFieldComponent implements OnChanges, AfterViewInit, OnDestroy {
  @Input() value = '';
  @Input() label?: string;
  @Input() placeholder?: string;
  @Input() type?: string;
  @Input() disabled = false;
  @Input() maxLength?: number;
  @Input() sensitive = false;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;
  @Input() nodeId?: string;
  @Output() valueChange = new EventEmitter<string>();
  @ViewChild('inputElement', { read: ElementRef }) inputElement?: ElementRef<HTMLInputElement>;

  localValue = '';
  showPassword = false;
  private isUserTyping = false;
  private typingTimeout?: ReturnType<typeof setTimeout>;

  constructor(private cdr: ChangeDetectorRef) {}

  ngAfterViewInit() {
    this.localValue = this.value || '';
  }

  ngOnDestroy() {
    if (this.typingTimeout) clearTimeout(this.typingTimeout);
  }

  get marginStyle() { return edgeInsetsToStyle(this.margin); }
  get paddingStyle() { return edgeInsetsToStyle(this.padding); }

  get inputType(): string {
    if (this.sensitive && !this.showPassword) return 'password';
    return this.type || 'text';
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
    setTimeout(() => this.inputElement?.nativeElement?.focus(), 0);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (!changes['value']) return;
    
    const newValue = changes['value'].currentValue || '';
    const isFirstChange = changes['value'].firstChange;
    
    if (isFirstChange || !this.isUserTyping) {
      const input = this.inputElement?.nativeElement;
      const hadFocus = input === document.activeElement;
      const { selectionStart, selectionEnd } = input || {};
      
      this.localValue = newValue;
      this.cdr.markForCheck();
      
      if (hadFocus && input && !this.isUserTyping) {
        setTimeout(() => {
          input.focus();
          if (selectionStart != null && selectionEnd != null) {
            input.setSelectionRange(selectionStart, selectionEnd);
          }
        }, 0);
      }
    }
  }

  onInput(event: Event) {
    this.isUserTyping = true;
    this.localValue = (event.target as HTMLInputElement).value;
    
    if (this.typingTimeout) clearTimeout(this.typingTimeout);
    this.valueChange.emit(this.localValue);
    this.typingTimeout = setTimeout(() => this.isUserTyping = false, 300);
  }

  onBlur() {
    this.isUserTyping = false;
    if (this.localValue !== this.value) {
      this.valueChange.emit(this.localValue);
    }
  }

  onEnter(event: Event) {
    this.isUserTyping = false;
    event.preventDefault();
    if (this.localValue !== this.value) {
      this.valueChange.emit(this.localValue);
    }
    (event.target as HTMLInputElement).blur();
  }
}
