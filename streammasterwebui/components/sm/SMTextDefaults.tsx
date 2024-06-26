interface SMTextColorProperties {
  readonly italicized?: boolean;
  readonly text: string | undefined; // Use a boolean to determine if text should be italicized
}
export const SMTextDefaults = ({ italicized, text }: SMTextColorProperties) => (
  <span className={`header-text-color text-xs ${italicized ? 'font-italic' : ''}`}>{text}</span>
);
