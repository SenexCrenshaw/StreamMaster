import useCopyToClipboard from '@lib/hooks/useCopyToClipboard';
import React, { useState } from 'react';
import BaseButton from './BaseButton';

export interface CopyButtonProperties {
  readonly disabled?: boolean | undefined;
  readonly notificationDuration?: number;
  readonly openCopyWindow: boolean;
  readonly value: string | undefined; // New prop for notification duration
}

const CopyButton: React.FC<CopyButtonProperties> = ({ disabled = false, value, openCopyWindow: openWindow = false, notificationDuration = 750 }) => {
  const [copied, setCopied] = useState(false);
  const [, copyToClipboard] = useCopyToClipboard(openWindow);

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
