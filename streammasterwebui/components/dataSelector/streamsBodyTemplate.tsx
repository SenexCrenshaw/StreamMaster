export function streamsBodyTemplate(activeCount: string, totalCount: string) {
  if (activeCount === null || totalCount === undefined) {
    return null;
  }

  return (
    <div className="flex align-items-center gap-2">
      {activeCount}/{totalCount}
    </div>
  );
}
