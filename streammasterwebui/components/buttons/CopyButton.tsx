import SMButton from '@components/sm/SMButton';
import useCopyToClipboard from '@lib/hooks/useCopyToClipboard';
import React, { useState } from 'react';
import { motion } from 'framer-motion';

export interface CopyButtonProperties {
  readonly buttonDisabled?: boolean | undefined;
  readonly notificationDuration?: number;
  readonly iconFilled?: boolean;
  readonly openCopyWindow: boolean;
  readonly value: string | undefined;
}

const CopyButton: React.FC<CopyButtonProperties> = ({
  buttonDisabled = false,
  value,
  iconFilled = false,
  openCopyWindow: openWindow = false,
  notificationDuration = 750
}) => {
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
    <div style={{ position: 'relative', display: 'inline-block' }}>
      <SMButton buttonDisabled={buttonDisabled} icon="pi-copy" buttonClassName="icon-blue" iconFilled={iconFilled} onClick={handleCopy} />
      <motion.span
        initial={{ opacity: 0, scale: 0.8 }} // Initial state before animation starts
        animate={{ opacity: copied ? 1 : 0, scale: copied ? 1 : 0.8 }} // Animate between these states
        transition={{ duration: 0.3, ease: 'easeInOut' }} // Animation duration and easing
        style={{
          position: 'absolute',
          top: '50%',
          left: '10%',
          transform: 'translate(-50%, -50%)',
          backgroundColor: 'rgba(0, 0, 0, 0.75)',
          color: 'white',
          padding: '4px 8px',
          borderRadius: '4px',
          pointerEvents: 'none',
          zIndex: 999
        }}
      >
        Copied!
      </motion.span>
    </div>
  );
};

export default CopyButton;
