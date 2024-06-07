using ApiFuncional.Data;
using ApiFuncional.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; //muitas coisas da parte de WebAPI ainda tem um conexão de dependencia do MVC pois tudo
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
// praticamente é filho de uma controller
namespace ApiFuncional.Controllers
{
    /*[Authorize]*/
    [ApiController] //Data Anotattion para dizer que se trata de uma controller de API
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        /*Injeção de dependência:
         * já vem embutida no Asp.Net, no contexto do EF mesmo nem precisamos declarar a injeção na 
         * Program.cs pois a própria definição que fizemos na Program.cs, informa o suficiente.
         */
        private readonly ApiDbContext _context; //criamos a propriedade privada _context, para receber a instânci de ApiDbContext 

        public ProdutosController(ApiDbContext context) //recebemos ApiDbContext no construtor
        {
            _context = context; //a propriedade privada _context está recebendo dados do contexto, assim criando a instância
        }
        //fim da injeção de dependência


        /* Criando os Métodos:
         * Primeiramente poderiamos tentar da forma abaixo, mas ela não vai ficar assim. 
         *
         * [HttpGet]
         * public IActionResult GetProdutos()
         * {
         * }
         * 
         * pois para retornarmos uma lista de produtos, vamos precisar do EF e com isso temos que fazer uma 
         * chamada Assincrona. Mudando a assinatura da nossa IActionResult, ficando da seguinte forma
        */
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        /*Sempre que trabalhamos com métodos assincronos o tipo de retorno definido vai ser sempre TASK na assinatura do método
* e na nossa implementação não podemos implementar a Interface de Action Result mas sim a própria classe, que é a mesma
* classe que implementa a interface. Depois passamos o que nossa ActionResult vai retornar, no caso uma coleção de 
* Produtos, ficando então Task<ActionResult<IEnumerable<Produto>>>
*/
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos() 
        {                                       
            if(_context.Produtos == null) //Evitando o erro de nullPointer exception, caso não exista a tabela Produtos
            {
                return NotFound();
            }

            return await _context.Produtos.ToListAsync(); //retornando uma lista de produtos assincrona
            //aqui não foi utilizado o result ok, pois com ou sem ele o resultado retornado vai ser um 200
            //eles são mais usados quando queremos retornar algo mais diferenciado
        }

        /*[AllowAnonymous]*/
        [HttpGet("{id:int}")] //recebendo o id pela rota
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> GetProduto(int id) //neste caso não é IEnumerable pois vai retornar apenas 1
        {
            if (_context.Produtos == null) //Evitando o erro de nullPointer exception caso não exista a tabela
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id); //FindAsync busca pela chave primaria da tabela

            if (produto == null) //Evitando o erro de nullPointer exception caso não exista o produto com o ID solicitado
            {
                return NotFound(); // se não existe retorna 404 not found
            }
            return produto;  
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto) //Mantemos o produto pois no nosso post também
        {                                                                     //queremos que retorne o produto cadastrado
            if(_context.Produtos == null)
            {
                return Problem("Erro ao criar um produto, contate o suporte!");
            }

            if(!ModelState.IsValid) //Algumas formas de fazer nossas próprias validações
            {
                //return BadRequest(ModelState); //funciona retorna os erros mas de forma simples

                //return ValidationProblem(ModelState); //este é a boa pratica recomendada.

                return ValidationProblem(new ValidationProblemDetails(ModelState) //outra versão da boa pratica, só que personalizada
                {
                    Title = "Um ou mais erros de validação ocorreram"
                });
            }

            _context.Produtos.Add(produto); //Adicionando o produto que recebemos via corpo no parametro do método
            await _context.SaveChangesAsync(); //Salvando o novo produto no banco de dados

            return CreatedAtAction(nameof(GetProduto),new {id = produto.Id}, produto); //retornando status 201
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if(id != produto.Id) return BadRequest();

            // validação do VS, diz que faz a validação automaticamente então não precisa dessa validação explicita, porém ele não sabe que removemos
            // esta validação, então precisamos manter o código abaixo
            if (!ModelState.IsValid) return ValidationProblem(ModelState); 

            //_context.Produtos.Update(produto); //Atualizando 1 produto
            _context.Entry(produto).State = EntityState.Modified;

            //await _context.SaveChangesAsync(); //Salvando alterações no banco de dados
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) 
            {
                if (!ProdutoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            if(_context.Produtos == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);

            if(produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto); //Removendo 1 produto
            await _context.SaveChangesAsync(); //Salvando alterações no banco de dados

            return NoContent();
        }

        private bool ProdutoExists(int id)
        {
            return (_context.Produtos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
