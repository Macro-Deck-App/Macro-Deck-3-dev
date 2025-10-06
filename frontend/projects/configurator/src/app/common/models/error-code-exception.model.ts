export class ErrorCodeExceptionModel extends Error {
  constructor(
    public readonly code: number,
    public override readonly message: string
  ) {
    super(message);
  }
}
