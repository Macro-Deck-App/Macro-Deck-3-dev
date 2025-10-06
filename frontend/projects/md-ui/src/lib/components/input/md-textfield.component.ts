import { Component, Input, Output, EventEmitter, ChangeDetectorRef, OnChanges, SimpleChanges, ViewChild, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EdgeInsets } from '../../models';

@Component({
  selector: 'md-textfield',
  template: `
    <div 
      [class]="customClasses || ''"
      [style]="customCss || ''"
      [style.margin]="marginStyle" 
      [style.padding]="paddingStyle">
      @if (label) {
        <label class="form-label">{{ label }}</label>
      }
      <input 
        #inputElement
        class="form-control"
        [type]="type || 'text'"
        [value]="localValue"
        (input)="onInput($event)"
        [placeholder]="placeholder || ''"
        [disabled]="disabled"
        [attr.maxlength]="maxLength"
        (blur)="onBlur()"
        (keydown.enter)="onEnter($event)" />
    </div>
  `,
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class MdTextFieldComponent implements OnChanges, AfterViewInit, OnDestroy {
  @Input() value: string = '';
  @Input() label?: string;
  @Input() placeholder?: string;
  @Input() type?: string;
  @Input() disabled: boolean = false;
  @Input() maxLength?: number;
  @Input() margin?: EdgeInsets;
  @Input() padding?: EdgeInsets;
  @Input() customCss?: string;
  @Input() customClasses?: string;
  @Input() nodeId?: string;
  @Output() valueChange = new EventEmitter<string>();
  @ViewChild('inputElement', { read: ElementRef }) inputElement?: ElementRef<HTMLInputElement>;

  localValue: string = '';
  private isUserTyping: boolean = false;
  private typingTimeout?: any;

  constructor(private cdr: ChangeDetectorRef) {}

  ngAfterViewInit(): void {
    // Initialize local value after view is ready
    this.localValue = this.value || '';
  }

  ngOnDestroy(): void {
    if (this.typingTimeout) {
      clearTimeout(this.typingTimeout);
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    // Update local value only if:
    // 1. It's the first change (initialization)
    // 2. The value from backend is different from our local value AND the user is not currently typing
    if (changes['value']) {
      const newValue = changes['value'].currentValue || '';
      const isFirstChange = changes['value'].firstChange;
      
      console.log('[MdTextField] Value change detected:', {
        nodeId: this.nodeId,
        newValue,
        localValue: this.localValue,
        isFirstChange,
        isUserTyping: this.isUserTyping,
        hasFocus: this.inputElement?.nativeElement === document.activeElement
      });
      
      // Only update if:
      // - First time (initialization)
      // - User is not typing (to avoid overwriting user input)
      if (isFirstChange || !this.isUserTyping) {
        const hadFocus = this.inputElement?.nativeElement === document.activeElement;
        const selectionStart = this.inputElement?.nativeElement?.selectionStart;
        const selectionEnd = this.inputElement?.nativeElement?.selectionEnd;
        
        this.localValue = newValue;
        this.cdr.markForCheck();
        
        // Restore focus and cursor position if it had focus
        if (hadFocus && this.inputElement?.nativeElement && !this.isUserTyping) {
          setTimeout(() => {
            if (this.inputElement?.nativeElement) {
              this.inputElement.nativeElement.focus();
              if (selectionStart !== null && selectionEnd !== null && 
                  selectionStart !== undefined && selectionEnd !== undefined) {
                this.inputElement.nativeElement.setSelectionRange(selectionStart, selectionEnd);
              }
            }
          }, 0);
        }
      }
    }
  }

  onInput(event: Event): void {
    this.isUserTyping = true;
    this.localValue = (event.target as HTMLInputElement).value;
    console.log('[MdTextField] User input:', { nodeId: this.nodeId, value: this.localValue });
    
    // Clear any existing timeout
    if (this.typingTimeout) {
      clearTimeout(this.typingTimeout);
    }
    
    // Emit immediately while typing
    this.valueChange.emit(this.localValue);
    
    // Reset isUserTyping after a short delay (e.g., 300ms after last keypress)
    this.typingTimeout = setTimeout(() => {
      this.isUserTyping = false;
      console.log('[MdTextField] User stopped typing:', { nodeId: this.nodeId });
    }, 300);
  }

  get marginStyle(): string | undefined {
    if (!this.margin) return undefined;
    return `${this.margin.top}px ${this.margin.right}px ${this.margin.bottom}px ${this.margin.left}px`;
  }

  get paddingStyle(): string | undefined {
    if (!this.padding) return undefined;
    return `${this.padding.top}px ${this.padding.right}px ${this.padding.bottom}px ${this.padding.left}px`;
  }

  onBlur(): void {
    this.isUserTyping = false;
    // Only emit if the value has actually changed
    if (this.localValue !== this.value) {
      console.log('[MdTextField] Emitting value on blur:', { nodeId: this.nodeId, value: this.localValue });
      this.valueChange.emit(this.localValue);
    }
  }

  onEnter(event: Event): void {
    this.isUserTyping = false;
    // Emit on Enter key
    event.preventDefault();
    if (this.localValue !== this.value) {
      console.log('[MdTextField] Emitting value on enter:', { nodeId: this.nodeId, value: this.localValue });
      this.valueChange.emit(this.localValue);
    }
    // Blur the input to remove focus
    (event.target as HTMLInputElement).blur();
  }
}
