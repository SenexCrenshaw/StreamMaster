interface SMTextColorProperties {
  readonly italicized?: boolean;
  readonly text: string | undefined; // Use a boolean to determine if text should be italicized
}
export const SMTextColor = ({ italicized, text }: SMTextColorProperties) => <span className={`orange-color ${italicized ? 'font-italic' : ''}`}>{text}</span>;
