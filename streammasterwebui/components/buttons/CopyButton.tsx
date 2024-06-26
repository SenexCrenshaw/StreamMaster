import SMButton from '@components/sm/SMButton';
import useCopyToClipboard from '@lib/hooks/useCopyToClipboard';
import React, { useState } from 'react';

export interface CopyButtonProperties {
  readonly buttonDisabled?: boolean | undefined;
  readonly notificationDuration?: number;
  readonly openCopyWindow: boolean;
  readonly value: string | undefined; // New prop for notification duration
}

const CopyButton: React.FC<CopyButtonProperties> = ({ buttonDisabled = false, value, openCopyWindow: openWindow = false, notificationDuration = 750 }) => {
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
      <SMButton buttonDisabled={buttonDisabled} icon="pi-copy" iconFilled={false} onClick={handleCopy} />
      {copied && <span className="copyButtonMessage">Copied!</span>}
    </div>
  );
};

export default CopyButton;
