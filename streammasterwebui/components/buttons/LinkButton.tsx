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

  const icon = bolt ? 'pi pi-bolt' : filled === true ? 'pi pi-bookmark-fill' : 'pi pi-bookmark';

  return (
    <div style={{ position: 'relative' }}>
      <div className="flex justify-content-center align-items-center">
        <a
          href={link}
          onClick={(e) => {
            e.preventDefault(); // Prevent default behavior (navigation)

            void copyToClipboard(link).then((ifCopied) => {
              setCopied(ifCopied);
              setTimeout(() => setCopied(false), 750);
            });
          }}
          rel="noopener noreferrer"
          target="_blank"
          title={title}
        >
          {/* Conditionally render the icon based on the copied state */}
          <i className={copied ? 'pi pi-copy' : icon} />
        </a>
      </div>
    </div>
  );
};
