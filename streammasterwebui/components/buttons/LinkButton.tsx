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

  const icon = bolt ? 'pi pi-bolt icon-primary' : filled === true ? 'pi pi-bookmark-fill icon-primary' : 'pi pi-bookmark icon-primary';

  return (
    <div className="flex justify-content-center align-items-center sm-button">
      <a
        href={link}
        onClick={(e) => {
          e.preventDefault();

          void copyToClipboard(link).then((ifCopied) => {
            setCopied(ifCopied);
            setTimeout(() => setCopied(false), 750);
          });
        }}
        rel="noopener noreferrer"
        target="_blank"
        title={title}
      >
        <i className={copied ? 'pi pi-copy icon-primary' : icon} />
      </a>
    </div>
  );
};
