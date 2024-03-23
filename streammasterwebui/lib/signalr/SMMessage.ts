export interface SMMessage {
  /**
   * Severity of the message.
   */
  severity?: 'success' | 'info' | 'warn' | 'error' | undefined;
  /**
   * Summary content of the message.
   */
  summary?: React.ReactNode | undefined;
  /**
   * Detail content of the message.
   */
  detail?: React.ReactNode | undefined;
  /**
   * Custom content of the message. If enabled, summary and details properties are ignored.
   */
  content?: React.ReactNode | undefined;
  /**
   * Whether the message can be closed manually using the close icon.
   * @defaultValue true
   */
  closable?: boolean | undefined;
  /**
   * When enabled, message is not removed automatically.
   */
  sticky?: boolean | undefined;
  /**
   * Delay in milliseconds to close the message automatically.
   * @defaultValue 3000
   */
  life?: number | undefined;
  /**
   * Style class of the message.
   */
  className?: string | undefined;
  /**
   * Inline style of the message.
   */
  style?: React.CSSProperties | undefined;
  /**
   * Style class of the message content.
   */
  contentClassName?: string | undefined;
  /**
   * Inline style of the message content.
   */
  contentStyle?: React.CSSProperties | undefined;
}
