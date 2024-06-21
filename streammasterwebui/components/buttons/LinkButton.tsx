import SMButton from '@components/sm/SMButton';
import useCopyToClipboard from '@lib/hooks/useCopyToClipboard';
import { useState } from 'react';

export const LinkButton = ({
  link,
  filled,
  title,
  bolt
}: {
  readonly link: string;
  readonly filled?: boolean;
  readonly title: string;
  readonly bolt?: boolean;
}) => {
  const [, copyToClipboard] = useCopyToClipboard(true);
  const [copied, setCopied] = useState(false);

  const icon = bolt ? 'pi-bolt' : filled === true ? 'pi-bookmark-fill' : 'pi-bookmark';

  return (
    <SMButton
      icon={copied ? 'pi-copy' : icon}
      buttonClassName={bolt ? 'icon-yellow' : 'icon-orange'}
      iconFilled={false}
      onClick={() => {
        copyToClipboard(link).then((ifCopied) => {
          setCopied(ifCopied);
          setTimeout(() => setCopied(false), 750);
        });
      }}
    />
  );
};
