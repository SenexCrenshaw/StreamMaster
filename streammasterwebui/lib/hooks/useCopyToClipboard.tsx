import { useState } from 'react';

type CopiedValue = string | null;
type CopyFn = (text: string) => Promise<boolean>; // Return success

const useCopyToClipboard = (): [CopiedValue, CopyFn] => {
  const [copiedText, setCopiedText] = useState<CopiedValue>(null);

  const copyToClipboard: CopyFn = async text => {
    if (!navigator?.clipboard) {
      console.warn('Clipboard not supported, enable SSL maybe?');
      window.open(text);

      return false;
    }

    // Try to save to clipboard then save it in the state if worked
    try {
      await navigator.clipboard.writeText(text);
      setCopiedText(text);

      return true;
    } catch (error) {
      console.warn('Copy failed', error);
      setCopiedText(null);

      return false;
    }
  };

  return [copiedText, copyToClipboard];
};

export default useCopyToClipboard;
