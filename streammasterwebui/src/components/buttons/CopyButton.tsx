;

import useCopyToClipboard from "../../hooks/useCopyToClipboard";
import BaseButton from "./BaseButton";

export type CopyButtonProps = {
  className?: string;
  disabled?: boolean | undefined;
  iconFilled?: boolean;
  label?: string | undefined;
  tooltip?: string;
  value: string | undefined;
}

const CopyButton: React.FC<CopyButtonProps> = ({ disabled = false, tooltip = "Copy to Clipboard", value }) => {
  const [, copyToClipboard] = useCopyToClipboard()
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-copy"
      iconFilled={false}
      onClick={() => { if (value !== undefined) void copyToClipboard(value); }}
      tooltip={tooltip}
    />
  );
};

export default CopyButton;
