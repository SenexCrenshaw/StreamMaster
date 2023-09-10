import React, { useState } from 'react';
import useCopyToClipboard from "../../hooks/useCopyToClipboard";
import BaseButton from "./BaseButton";

export type CopyButtonProps = {
  readonly disabled?: boolean | undefined;
  readonly notificationDuration?: number;
  readonly tooltip?: string;
  readonly value: string | undefined;  // New prop for notification duration
}

const CopyButton: React.FC<CopyButtonProps> = ({
  disabled = false,
  tooltip = "Copy to Clipboard",
  value,
  notificationDuration = 750
}) => {
  const [copied, setCopied] = useState(false);
  const [, copyToClipboard] = useCopyToClipboard();

  const handleCopy = () => {
    if (value !== undefined) {
      void copyToClipboard(value).then(ifCopied => {
        setCopied(ifCopied)
        setTimeout(() => setCopied(false), notificationDuration);
      }
      );
    }
  };

  return (
    <div style={{ position: 'relative' }}>
      <BaseButton
        disabled={disabled}
        icon="pi-copy"
        iconFilled={false}
        onClick={handleCopy}
        tooltip={tooltip}
      />
      {copied &&
        <span className='copyButtonMessage'>Copied!</span>
      }
    </div>
  );
};

export default CopyButton;
