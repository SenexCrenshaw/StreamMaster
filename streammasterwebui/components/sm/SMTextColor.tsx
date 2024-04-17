interface SMTextColorProperties {
  readonly italicized?: boolean;
  readonly text: string | undefined; // Use a boolean to determine if text should be italicized
}
export const SMTextColor = ({ italicized, text }: SMTextColorProperties) => (
  <span className={`header-text-color text-sm ${italicized ? 'font-italic' : ''}`}>{text}</span>
);
