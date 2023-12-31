import { useState } from 'react';

type CopiedValue = string | null;
interface CopyFunction {
  (text: string): Promise<boolean>;
} // Return success

const useCopyToClipboard = (openCopyWindow: boolean): [CopiedValue, CopyFunction] => {
  const [copiedText, setCopiedText] = useState<CopiedValue>(null);

  const copyToClipboard: CopyFunction = async (text) => {
    if (!navigator?.clipboard) {
      if (openCopyWindow) {
        window.open(text);
      }
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
