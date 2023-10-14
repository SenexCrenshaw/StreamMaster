import useCopyToClipboard from '@lib/hooks/useCopyToClipboard';
import React, { useState } from 'react';
import BaseButton from './BaseButton';

export type CopyButtonProps = {
  readonly disabled?: boolean | undefined;
  readonly notificationDuration?: number;
  readonly value: string | undefined; // New prop for notification duration
};

const CopyButton: React.FC<CopyButtonProps> = ({ disabled = false, value, notificationDuration = 750 }) => {
  const [copied, setCopied] = useState(false);
  const [, copyToClipboard] = useCopyToClipboard();

  const handleCopy = () => {
    if (value !== undefined) {
      void copyToClipboard(value).then((ifCopied) => {
        setCopied(ifCopied);
        setTimeout(() => setCopied(false), notificationDuration);
      });
    }
  };

  return (
    <div style={{ position: 'relative' }}>
      <BaseButton disabled={disabled} icon="pi-copy" iconFilled={false} onClick={handleCopy} />
      {copied && <span className="copyButtonMessage">Copied!</span>}
    </div>
  );
};

export default CopyButton;
