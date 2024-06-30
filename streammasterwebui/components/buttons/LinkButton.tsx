import useCopyToClipboard from '@lib/hooks/useCopyToClipboard';
import { useState } from 'react';

export const LinkButton = ({
  link,
  filled,
  bolt,
  title,
  justText
}: {
  readonly link: string;
  readonly filled?: boolean;
  readonly title: string;
  readonly bolt?: boolean;
  readonly justText?: boolean;
}) => {
  const [, copyToClipboard] = useCopyToClipboard(true);
  const [copied, setCopied] = useState(false);

  const icon = bolt ? 'pi pi-bolt icon-yellow' : filled === true ? 'pi pi-bookmark-fill icon-orange' : 'pi pi-bookmark icon-orange';

  return (
    <div className="p-button-icon-only flex align-items-center justify-content-center">
      <a
        href={link}
        onClick={(e) => {
          e.preventDefault();

          void copyToClipboard(link.replace(/"/g, '')).then((ifCopied) => {
            setCopied(ifCopied);
            setTimeout(() => setCopied(false), 750);
          });
        }}
        rel="noopener noreferrer"
        target="_blank"
        title={title}
      >
        {justText === true ? <div className="sm-w-12rem">{title}</div> : <i className={copied ? 'pi pi-copy' : icon} />}
      </a>
    </div>
  );
};
