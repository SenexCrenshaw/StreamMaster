import SMButton from '@components/sm/SMButton';
import { Logger } from '@lib/common/logger';
import useCopyToClipboard from '@lib/hooks/useCopyToClipboard';
import { useState } from 'react';

export const LinkButton = ({ link, filled, bolt }: { readonly link: string; readonly filled?: boolean; readonly title: string; readonly bolt?: boolean }) => {
  const [, copyToClipboard] = useCopyToClipboard(true);
  const [copied, setCopied] = useState(false);

  const icon = bolt ? 'pi-bolt' : filled === true ? 'pi-bookmark-fill' : 'pi-bookmark';

  return (
    <SMButton
      icon={copied ? 'pi-copy' : icon}
      buttonClassName={bolt ? 'icon-yellow' : 'icon-orange'}
      iconFilled={false}
      onClick={() => {
        Logger.debug('LinkButton', 'onClick', link.replace(/"/g, ''));
        copyToClipboard(link.replace(/"/g, '')).then((ifCopied) => {
          setCopied(ifCopied);
          setTimeout(() => setCopied(false), 750);
        });
      }}
    />
  );
};
