import useCopyToClipboard from "../../hooks/useCopyToClipboard";

export const LinkButton = ({ link }: { readonly link: string }) => {
  const [, copyToClipboard] = useCopyToClipboard();

  return (
    <div className="flex justify-content-center align-items-center">
      <a
        href={link}
        onClick={(e) => {
          e.preventDefault(); // Prevent default behavior (navigation)
          void copyToClipboard(link);
        }}
        rel="noopener noreferrer"
        target="_blank"
        title="Copy link to clipboard"
      >
        <i className="pi pi-bookmark-fill" />
      </a>
    </div>
  );
}


